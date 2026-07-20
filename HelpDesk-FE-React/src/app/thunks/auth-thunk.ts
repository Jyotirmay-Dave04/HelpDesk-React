import { createAsyncThunk } from "@reduxjs/toolkit";
import { loginAsync } from "../../services/auth-service";
import type { LoginRequest } from "../../interfaces/login";

export const login = createAsyncThunk(
    'auth/login',
    async (creads: LoginRequest) => {
        const response = await loginAsync(creads);
        return response;
    }
)