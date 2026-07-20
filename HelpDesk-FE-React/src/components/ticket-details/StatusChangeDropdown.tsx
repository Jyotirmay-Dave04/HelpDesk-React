import { useState } from "react";
import type { useConfirm } from "../../app/hooks/confirm-hook";
import { TicketStatus } from "../../types/ticket-status";
import { requiresReason } from "../../utils/TicketTransitions";
import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";

interface Props {
    options: TicketStatus[];
    disabled?: boolean;
    onConfirmed: (target: TicketStatus, reason?: string) => void;
    confirm: ReturnType<typeof useConfirm>['confirm']; // shared instance from parent
}

const StatusChangeDropdown = ({ options, disabled, onConfirmed, confirm }: Props) => {
    const [value, setValue] = useState<TicketStatus | ''>('');

    if (options.length === 0) return null;

    const handleChange = async (target: TicketStatus) => {
        setValue(target);
        const result = await confirm({
            title: `Change status to ${target}`,
            message: `Are you sure you want to move this ticket to "${target}"?`,
            requireReason: requiresReason(target),
            confirmText: 'Update Status',
        });

        if (result.confirmed) {
            onConfirmed(target, result.reason);
        }
        setValue('');
    };

    return (
        <FormControl size="small" sx={{ minWidth: 180 }} disabled={disabled}>
            <InputLabel id="status-change-label">Change Status</InputLabel>
            <Select
                labelId="status-change-label"
                label="Change Status"
                value={value}
                onChange={(e) => handleChange(e.target.value as TicketStatus)}
            >
                {options.map((status) => (
                    <MenuItem key={status} value={status}>
                        {status}
                    </MenuItem>
                ))}
            </Select>
        </FormControl>
    );
};

export default StatusChangeDropdown;