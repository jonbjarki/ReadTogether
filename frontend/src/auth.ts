import NextAuth from 'next-auth';
import { authConfig } from '../auth.config';
import Google from 'next-auth/providers/google';

const API_URL = process.env.API_URL!;

type GoogleAuthResponse = {
    userId: string,
    email: string,
    userName?: string,
    jwtToken: string,
    expiresAt: Date
}

const AUTHORIZED_PATHS = ["/users/", "/bookshelves/"];

export const { handlers, auth, signIn, signOut } = NextAuth({
    ...authConfig,
    providers: [Google],
    session: {
        strategy: "jwt",
    },
    callbacks: {
        authorized: async ({ request, auth }) => {
            // Logged in users are authenticated, otherwise redirect to login page
            const isLoggedIn = !!auth;
            if (AUTHORIZED_PATHS.some(path => request.nextUrl.pathname.startsWith(path))) {
                console.log(`FIRED with path: ${AUTHORIZED_PATHS.find(path => request.nextUrl.pathname.startsWith(path))}`);
                console.log(request.nextUrl.pathname);
                return isLoggedIn;
            }
            return true;
        },
        async jwt({ token, account, profile }) {
            if (!account) {
                return token;
            }

            const idToken = account?.id_token;
            if (!idToken || !profile?.email) {
                console.error("Missing Google account/profile data during sign-in", {
                    hasIdToken: !!idToken,
                    hasEmail: !!profile?.email,
                });
                return token;
            }
            console.log("ID TOKEN=", idToken);

            // Authenticate with backend and creates the user if it does not exists
            // Backend returns custom JWT for authenticating future requests
            console.log("Making request to: ", API_URL + "auth/google")
            console.log("With body: ", JSON.stringify({ "idToken": idToken }));

            const res = await fetch(API_URL + "auth/google", {
                method: "POST",
                body: JSON.stringify({ idToken: idToken }),
                headers: {
                    "Content-Type": "application/json"
                },
                credentials: "include"
            });
            console.log("RES:", res);

            if (!res.ok || res.status >= 400) {
                console.error("Failed to authenticate with backend", res.status, await res.text());
                return token;
            }

            const data = await res.json() as GoogleAuthResponse;

            token.accessToken = data.jwtToken;
            token.sub = data.userId;
            token.email = data.email;
            token.name = data.userName || "";
            return token;
        },
        async session({ session, token }) {
            // We do not include the access token in the session to avoid exposing it to the client.
            return {
                ...session,
                user: {
                    ...session.user,
                    id: token.sub || "",
                    email: token.email || "",
                    name: token.name || ""
                }
            }
        }
    }
});