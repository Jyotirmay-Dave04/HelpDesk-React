import axios from "axios";
import { toast } from "../utils/Toast";

const axiosInstance = axios.create({
    baseURL: "http://localhost:5089/api",
    headers: {
        "Content-Type": "application/json",
    },
});

axiosInstance.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

axiosInstance.interceptors.response.use(
    (response) => response,
    (error) => {        
        const isLoginRequest = error.config?.url?.includes('/Auth/login');
        if(error.response.status === 401 && !isLoginRequest){
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