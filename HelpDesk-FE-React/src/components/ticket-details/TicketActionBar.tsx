import { useState } from "react";
import { useConfirm } from "../../app/hooks/confirm-hook";
import { TicketStatus } from "../../types/ticket-status";
import type { UserRole } from "../../types/user-role";
import { canAssign, canCancel, canClose, canReject, canReopen, getDropdownTransitions } from "../../utils/TicketTransitions";
import { Stack, Button } from "@mui/material";
import { updateTicketStatusAsync, assignTicketAsync } from "../../services/ticket-service";
import { toast } from "../../utils/Toast";
import AssignTicketDialog from "./AssignTicketDialog";
import StatusChangeDropdown from "./StatusChangeDropdown";
import { Block, Cancel, Close, PersonAdd, Repeat } from "@mui/icons-material";

interface Props {
    ticketId: number;
    status: TicketStatus;
    assignedAgentId?: number | null;
    assignedAgentName?: string | null;
    role: UserRole;
    onActionComplete: () => void;
}

const TicketActionsBar = ({ ticketId, status, assignedAgentId, assignedAgentName, role, onActionComplete }: Props) => {
    const { confirm, ConfirmDialogUI } = useConfirm();
    const [assignDialogOpen, setAssignDialogOpen] = useState(false);
    const [submitting, setSubmitting] = useState(false);

    const showReject = canReject(role, status);
    const showReopen = canReopen(role, status);
    const showCancel = canCancel(role, status);
    const showAssign = canAssign(role, status);
    const showClose = canClose(role, status);
    const dropdownOptions = getDropdownTransitions(role, status);

    if (!showReject && !showCancel && !showAssign && !showReopen && !showClose && dropdownOptions.length === 0) return null;

    const runStatusUpdate = async (newStatus: TicketStatus, reason?: string) => {
        setSubmitting(true);
        try {
            const response = await updateTicketStatusAsync(ticketId, { status: newStatus, reason });
            toast.success(response.message);
            onActionComplete();
        } catch (error) {
            toast.error(error);
        } finally {
            setSubmitting(false);
        }
    };

    const handleReject = async () => {
        const result = await confirm({
            title: 'Reject Ticket',
            message: 'This will mark the ticket as rejected.',
            confirmText: 'Reject',
        });
        if (result.confirmed) await runStatusUpdate(TicketStatus.Rejected);
    };

    const handleReopen = async () => {
        const result = await confirm({
            title: 'Re-open Ticket',
            message: 'This will re open the ticket.',
            confirmText: 'Reopen'
        });
        if (result.confirmed) await runStatusUpdate(TicketStatus.ReOpen);
    }

    const handleCancel = async () => {
        const result = await confirm({
            title: 'Cancel Ticket',
            message: 'This will cancel the ticket.',
            confirmText: 'Cancel Ticket',
        });
        if (result.confirmed) await runStatusUpdate(TicketStatus.Cancelled);
    };

    const handleClose = async () => {
        const result = await confirm({
            title: 'Close Ticket',
            message: 'This will close the ticket.',
            confirmText: 'Close Ticket'
        });
        if (result.confirmed) await runStatusUpdate(TicketStatus.Closed);
    }

    const handleAssignClose = async (result: { confirmed: boolean; agentId?: number }) => {
        setAssignDialogOpen(false);
        if (!result.confirmed || result.agentId === undefined) return;
        setSubmitting(true);
        try {
            const response = await assignTicketAsync(ticketId, { assignedToId: result.agentId });
            toast.success(response.message);
            onActionComplete();
        } catch (error) {
            toast.error(error);
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <>
            <Stack direction="row" spacing={1.5} sx={{ alignItems: 'center', flexWrap: 'wrap' }} useFlexGap>
                {showAssign && (
                    <Button
                        variant="outlined"
                        startIcon={<PersonAdd />}
                        disabled={submitting}
                        onClick={() => setAssignDialogOpen(true)}
                    >
                        {assignedAgentId ? 'Reassign' : 'Assign'}
                    </Button>
                )}

                {dropdownOptions.length > 0 && (
                    <StatusChangeDropdown
                        options={dropdownOptions}
                        disabled={submitting}
                        onConfirmed={runStatusUpdate}
                        confirm={confirm}
                    />
                )}

                {showReject && (
                    <Button variant="outlined" color="error" startIcon={<Block />} disabled={submitting} onClick={handleReject}>
                        Reject
                    </Button>
                )}

                {showReopen && (
                    <Button variant='outlined' color="success" startIcon={<Repeat />} disabled={submitting} onClick={handleReopen}>
                        Reopen
                    </Button>
                )}

                {showClose && (
                    <Button variant='outlined' startIcon={<Close />} disabled={submitting} onClick={handleClose}>
                        Close 
                    </Button>
                )}

                {showCancel && (
                    <Button variant="outlined" color="warning" startIcon={<Cancel />} disabled={submitting} onClick={handleCancel}>
                        Cancel
                    </Button>
                )}
            </Stack>

            {ConfirmDialogUI}

            <AssignTicketDialog
                open={assignDialogOpen}
                currentAgent={
                    assignedAgentId && assignedAgentName
                        ? { id: assignedAgentId, name: assignedAgentName }
                        : null
                }
                onClose={handleAssignClose}
            />
        </>
    );
};

export default TicketActionsBar;