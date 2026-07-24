import type { NotificationType } from "../types/notification";

export interface NotificationResponse {
    id: number;
    ticketId: number | null;
    type: NotificationType;
    message: string;
    isRead: boolean;
    createdAt: string;
}

export interface NotificationRequestFilter {
    page?: number;
    pageSize?: number;
}