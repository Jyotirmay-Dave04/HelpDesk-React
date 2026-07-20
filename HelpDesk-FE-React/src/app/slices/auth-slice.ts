import { createSlice } from "@reduxjs/toolkit";
import type { User } from "../../interfaces/jwt-user";
import { jwtDecode } from "jwt-decode";
import type { DecodedToken } from "../../interfaces/decoded-token";
import { login } from "../thunks/auth-thunk";

interface AuthState {
    token: string | null;
    user: User | null;
    isAuthenticated: boolean;
    loading: boolean;
    error: string | null;
}

const initialState: AuthState = {
    token: localStorage.getItem('token') ?? null,
    user: localStorage.getItem('token') ? decodeUser(localStorage.getItem('token')) : null,
    isAuthenticated: !!localStorage.getItem('token'),
    loading: false,
    error: null
}

function decodeUser(token: string): User | null {
    try{
        const decoded = jwtDecode<DecodedToken>(token);
        const isExpired = decoded.exp * 1000 < Date.now();
        if(isExpired) return null;
        return {
            id: parseInt(decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]),
            name: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
            email: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
            role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
        }
    } catch {
        return null;
    }
}

export const authSlice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        logout: (state) => {
            state.isAuthenticated = false;
            state.token = null;
            state.user = null;
            localStorage.removeItem('token');
        }
    },
    extraReducers: (builder) => {
        builder.addCase(
            login.pending,
            (state) => {
                state.loading = true;
            }
        ),
        builder.addCase(
            login.fulfilled,
            (state, action) => {
                const newToken = action.payload.data.token;
                state.token = newToken;
                localStorage.setItem('token', newToken);
                state.user = decodeUser(newToken);
                state.isAuthenticated = true;
                state.error = null;
                state.loading = false;
            }
        ),
        builder.addCase(
            login.rejected,
            (state, action) => {
                state.token = null;
                state.user = null;
                state.isAuthenticated = false;
                localStorage.removeItem('token');
                state.error = action.error.message ?? "Something went wrong";
                state.loading = false;
            }
        )
    }
});

export const { logout } = authSlice.actions;

export default authSlice.reducer;