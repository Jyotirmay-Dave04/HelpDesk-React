import axiosInstance from "../axios/axios-instance";
import { ApiEndpoints } from "../constants/api-endpoints";
import type { ApiResponse } from "../interfaces/api-response";
import type { PagedResponse } from "../interfaces/paged-response";
import type { AssignTicketPayload, TicketCreate, TicketFilter, TicketListItem, TicketResponse, UpdateTicketPayload, UpdateTicketStatusPayload } from "../interfaces/ticket";

export const getAllTicketsAsync = async (filter: TicketFilter): Promise<ApiResponse<PagedResponse<TicketListItem>>> => {
    const response = await axiosInstance.post<ApiResponse<PagedResponse<TicketListItem>>>(ApiEndpoints.TICKETS.GETALLWITHPOST, filter);
    return response.data;
}

export const createTicketAsync = async (request: TicketCreate): Promise<ApiResponse<TicketResponse>> => {
    const response = await axiosInstance.post<ApiResponse<TicketResponse>>(ApiEndpoints.TICKETS.CREATE, request);
    return response.data;
}

export const getTicketByIdAsync = async (id: number): Promise<ApiResponse<TicketResponse>> => {
    const response = await axiosInstance.get<ApiResponse<TicketResponse>>(ApiEndpoints.TICKETS.BY_ID(id));
    return response.data;
}

export const updateTicketAsync = async (id: number, body: UpdateTicketPayload): Promise<ApiResponse<TicketResponse>> => {
    const response = await axiosInstance.put<ApiResponse<TicketResponse>>(ApiEndpoints.TICKETS.UPDATE(id), body);
    return response.data;
}

export const deleteTicketAsync = async (id: number): Promise<ApiResponse<boolean>> => {
    const response = await axiosInstance.delete<ApiResponse<boolean>>(ApiEndpoints.TICKETS.DELETE(id));
    return response.data;
}

export const assignTicketAsync = async (id: number, payload: AssignTicketPayload): Promise<ApiResponse<TicketResponse>> => {
    const response = await axiosInstance.put<ApiResponse<TicketResponse>>(ApiEndpoints.TICKETS.ASSIGN(id), payload);
    return response.data;
}

export const updateTicketStatusAsync = async (id: number, payload: UpdateTicketStatusPayload): Promise<ApiResponse<TicketResponse>> => {
    const response = await axiosInstance.put<ApiResponse<TicketResponse>>(ApiEndpoints.TICKETS.STATUS(id), payload);
    return response.data;
}