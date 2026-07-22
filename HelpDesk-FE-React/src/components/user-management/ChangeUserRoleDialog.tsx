import { useEffect, useState } from "react";
import { UserRole } from "../../types/user-role";
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, InputLabel, MenuItem, Select } from "@mui/material";

interface ChangeUserRoleDialogProps {
    open: boolean;
    currentRole: UserRole;
    userName: string;
    onClose: () => void;
    onConfirm: (newRole: UserRole) => void;
    loading?: boolean;
}

export function ChangeUserRoleDialog({
    open, currentRole, userName, loading, onClose, onConfirm
}: ChangeUserRoleDialogProps) {
    const [selectedRole, setSelectedRole] = useState<UserRole>(currentRole);

    useEffect(() => {
        if (open) setSelectedRole(currentRole);
    }, [open, currentRole]);

    return (
        <Dialog open={open} onClose={onclose} fullWidth maxWidth='xs'>
            <DialogTitle>Change Role - {userName}</DialogTitle>
            <DialogContent>
                <FormControl fullWidth margin='dense'>
                    <InputLabel id='role-select-label'>Role</InputLabel>
                    <Select
                        labelId="role-select-label"
                        value={selectedRole}
                        label="Role"
                        onChange={(e) => setSelectedRole(e.target.value as UserRole)}
                    >
                        {Object.values(UserRole).map((role) => (
                            <MenuItem key={role} value={role}>{role}</MenuItem>
                        ))}
                    </Select>
                </FormControl>
            </DialogContent>
            <DialogActions>
                <Button disabled={loading} onClick={onClose}>Cancel</Button>
                <Button variant='contained' disabled={loading || selectedRole === currentRole} onClick={() => onConfirm(selectedRole)}>Confirm</Button>
            </DialogActions>
        </Dialog>
    );
}