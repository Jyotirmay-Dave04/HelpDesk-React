import axios from "axios";
import { toast } from "../utils/Toast";
import { decryptPayload, encryptPayload } from "../utils/CryptoUtils";

const axiosInstance = axios.create({
    baseURL: "https://localhost:7150/api",
    headers: {
        "Content-Type": "application/json",
    },
});

axiosInstance.interceptors.request.use(
    async (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        if (config.data && ['post', 'put', 'patch'].includes(config.method ?? '')) {
            const encrypted = await encryptPayload(config.data);
            config.data = encrypted;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

axiosInstance.interceptors.response.use(
    async (response) => {
        if (response.data) {
            response.data = await decryptPayload(response.data);
        }
        return response;
    },
    async (error) => {
        if (!error.response) {
            // network error, CORS block, server unreachable
            toast.error("Cannot reach server. Please check your connection.");
            return Promise.reject({ message: "Network error" });
        }

        if (error.response?.data) {
            try {
                error.response.data = await decryptPayload(error.response.data);
            } catch {}
        }
        
        const isLoginRequest = error.config?.url?.includes('/Auth/login');
        if (error.response.status === 401 && !isLoginRequest) {
            toast.error("Session expired. Please log in again.");
            localStorage.removeItem('token');
            window.location.href = "/login";
        } else if (error.response.status === 500) {
            toast.error("Something went wrong on our end. Please try again.");
        }
        return Promise.reject(error.response.data);
    }
);

export default axiosInstance;