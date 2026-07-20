import { Priority } from "../types/priority";
import { TicketStatus } from "../types/ticket-status";

type ChipColor = 'default' | 'primary' | 'secondary' | 'error' | 'warning' | 'info' | 'success';

export const getStatusColor = (status: TicketStatus): ChipColor => {
  switch (status) {
    case TicketStatus.Open: return 'info';
    case TicketStatus.InProgress: return 'primary';
    case TicketStatus.OnHold: return 'warning';
    case TicketStatus.Resolved: return 'success';
    case TicketStatus.Cancelled: return 'default';
    case TicketStatus.ReOpen: return 'secondary';
    case TicketStatus.CannotResolve: return 'error';
    default: return 'default';
  }
};

export const getPriorityColor = (priority: Priority): ChipColor => {
  switch (priority) {
    case Priority.High: return 'error';
    case Priority.Medium: return 'warning';
    case Priority.Low: return 'info';
    default: return 'default';
  }
};

export const formatDuration = (totalSeconds: number): string => {
    if (totalSeconds < 60) return `${totalSeconds}s`;

    const days = Math.floor(totalSeconds / 86400);
    const hours = Math.floor((totalSeconds % 86400) / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    let ans = "";

    if (days > 0) ans += `${days}d `;
    if (hours > 0) ans += `${hours}h `;
    if (minutes > 0) ans += `${minutes}m `;
    return ans.trim();
}