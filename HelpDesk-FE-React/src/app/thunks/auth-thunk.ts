import { createAsyncThunk } from "@reduxjs/toolkit";
import { loginAsync, registerAsync } from "../../services/auth-service";
import type { LoginRequest, RegisterRequest } from "../../interfaces/login";

export const login = createAsyncThunk(
    'auth/login',
    async (creads: LoginRequest) => {
        return await loginAsync(creads);
    }
);

export const register = createAsyncThunk(
    'auth/register',
    async (creads: RegisterRequest) => {
        return await registerAsync(creads);
    }
);