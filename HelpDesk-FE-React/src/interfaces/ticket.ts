import { Priority } from '../types/priority';
import { TicketStatus } from '../types/ticket-status';

export interface TicketCreate {
    groupId: number | '';
    categoryId: number | '';
    subCategoryId: number | '';
    priority: Priority | '';
    serviceDetails: string;
}

export interface TicketListItem {
    id: number;
    serviceDetails: string;
    status: TicketStatus;
    priority: Priority;
    groupName: string;
    categoryNmae: string;
    subCategoryName: string;
    requesterName: string;
    assignedToName: string | null;
    resolvedAt: string | null;
    slaDeadline: string;
    slaBreached: boolean;
    createdAt: string;
}

export interface TicketResponse {
    id: number;
    serviceDetails: string;
    status: TicketStatus;
    priority: Priority;
    groupId: number;
    groupName: string;
    categoryId: number;
    categoryName: string;
    subCategoryId: number;
    subCategoryName: string;
    requestedById: number;
    requesterName: string;
    assignedToId: number | null;
    assignedToName: string | null;
    resolvedAt: string | null;
    slaDeadline: string;
    slaBreached: boolean;
    isSlaPaused: boolean;
    onHoldPausedSeconds: number;
    postResolutionPausedSeconds: number;
    breachedBeforeReopen: boolean;
    reopenCount: number;
    createdAt: string;
    modifiedAt: string | null;
}

export interface TicketFilter {
    search?: string;
    status?: string;
    priority?: string;
    groupId?: number;
    categoryId?: number;
    assignedTo?: number;
    breached?: boolean;
    dateFrom?: string;
    dateTo?: string;
    sortBy?: string;
    sortDirection?: 'asc' | 'desc';
    page: number;
    pageSize: number;
}

export interface UpdateTicketPayload {
    priority: Priority | '';
    serviceDetails: string;
}

export interface AssignTicketPayload {
    assignedToId: number;
}

export interface UpdateTicketStatusPayload {
    status: string;
    reason?: string;
}