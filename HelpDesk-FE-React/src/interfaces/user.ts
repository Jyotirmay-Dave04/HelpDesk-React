import type { UserRole } from "../types/user-role";

export interface UserListItem {
    id: number;
    name: string;
    email: string;
    role: UserRole;
    activeTicketCount?: number;
    createdAt: string;
}

export interface UserFilter {
    role?: UserRole;
    search?: string;
    sortBy?: string;
    sortDirection?: 'asc' | 'desc';
    page?: number;
    pageSize?: number;
}

export interface ChangeRolePayload {
    role: UserRole;
}

export interface AssignAgentRequest {
    search?: string;
    page?: number;
    pageSize?: number;
}

export type AgentOption = Pick<UserListItem, 'id' | 'name'>;