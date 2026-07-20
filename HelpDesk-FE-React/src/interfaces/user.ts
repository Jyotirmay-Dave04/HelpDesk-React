export interface UserListItem {
    id: number;
    name: string;
    email: string;
    role: string;
    activeTicketCount?: number;
    createdAt: string;
}

export interface AssignAgentRequest {
    search?: string;
    page?: number;
    pageSize?: number;
}

export type AgentOption = Pick<UserListItem, 'id' | 'name'>;