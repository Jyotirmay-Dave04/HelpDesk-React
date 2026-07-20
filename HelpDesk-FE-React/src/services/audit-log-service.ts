import axiosInstance from "../axios/axios-instance";
import { ApiEndpoints } from "../constants/api-endpoints";
import type { ApiResponse } from "../interfaces/api-response";
import type { AuditLog } from "../interfaces/audit-log";

export const getAuditLogsByTicketIdAsync = async (id: number): Promise<ApiResponse<AuditLog[]>> => {
    const response = await axiosInstance.get<ApiResponse<AuditLog[]>>(ApiEndpoints.AUDITLOGS.BY_TICKET(id));
    return response.data;
}