import { useEffect, useState } from "react";
import type { AgentOption } from "../../interfaces/user";
import { Button, Dialog, DialogActions, DialogContent, DialogTitle } from "@mui/material";
import AgentAutocomplete from "./AgentAutoComplete";

interface Props {
    open: boolean;
    currentAgent?: AgentOption | null;
    onClose: (result: { confirmed: boolean; agentId?: number }) => void;
}

const AssignTicketDialog = ({ open, currentAgent, onClose }: Props) => {
    const [selectedAgent, setSelectedAgent] = useState<AgentOption | null>(currentAgent ?? null);

    useEffect(() => {
        if (open) setSelectedAgent(currentAgent ?? null);
    }, [open, currentAgent]);

    const handleConfirm = () => {
        if (!selectedAgent) return;
        onClose({ confirmed: true, agentId: selectedAgent.id });
    };

    const handleCancel = () => onClose({ confirmed: false });

    return (
        <Dialog open={open} onClose={handleCancel} maxWidth="xs" fullWidth>
            <DialogTitle>Assign Ticket</DialogTitle>
            <DialogContent>
                <AgentAutocomplete value={selectedAgent} onChange={setSelectedAgent} />
            </DialogContent>
            <DialogActions>
                <Button onClick={handleCancel}>Cancel</Button>
                <Button onClick={handleConfirm} variant="contained" disabled={!selectedAgent}>
                    Assign
                </Button>
            </DialogActions>
        </Dialog>
    );
};

export default AssignTicketDialog;