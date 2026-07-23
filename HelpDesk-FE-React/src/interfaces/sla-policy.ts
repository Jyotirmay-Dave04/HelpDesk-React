import type { Priority } from "../types/priority";

export interface SlaPolicyResponse {
    id: number;
    priority: Priority;
    hoursToResolve: number;
}

export interface UpdateSlaPolicyPayload {
    hoursToResolve: number;
}