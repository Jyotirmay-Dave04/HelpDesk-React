import axiosInstance from "../axios/axios-instance";
import { ApiEndpoints } from "../constants/api-endpoints";
import type { ApiResponse } from "../interfaces/api-response";
import type { Comment, CreateCommentPayload } from "../interfaces/comment";

export const getCommentsByTicketIdAsync = async (id: number): Promise<ApiResponse<Comment[]>> =>  {
    const response = await axiosInstance.get<ApiResponse<Comment[]>>(ApiEndpoints.COMMENTS.BY_TICKET(id));
    return response.data;
}

export const createCommentAsync = async (payload: CreateCommentPayload): Promise<ApiResponse<Comment>> => {
    const response = await axiosInstance.post<ApiResponse<Comment>>(ApiEndpoints.COMMENTS.CREATE, payload);
    return response.data;
}