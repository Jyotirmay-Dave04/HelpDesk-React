import { Dialog, DialogTitle, DialogContent, DialogContentText, DialogActions, Button, TextField } from "@mui/material";
import { useState, useRef, useCallback } from "react";

interface ConfirmOptions {
    title: string;
    message: string;
    requireReason?: boolean;
    confirmText?: string;
    cancelText?: string;
}

interface ConfirmResult {
    confirmed: boolean;
    reason?: string;
}

export function useConfirm() {
    const [options, setOptions] = useState<ConfirmOptions | null>(null);
    const [reason, setReason] = useState('');
    const resolveRef = useRef<(result: ConfirmResult) => void>(null);

    const confirm = useCallback((opts: ConfirmOptions): Promise<ConfirmResult> => {
        setOptions(opts);
        setReason("");
        return new Promise((resolve) => {
            resolveRef.current = resolve;
        });
    }, []);

    const handleClose = (result: ConfirmResult) => {
        resolveRef.current?.(result);
        setOptions(null);
    };

    const ConfirmDialogUI = options ? (
        <Dialog open onClose={() => handleClose({ confirmed: false })}>
            <DialogTitle>{options.title}</DialogTitle>
            <DialogContent>
                <DialogContentText>{options.message}</DialogContentText>
                {options.requireReason && (
                    <TextField
                        autoFocus
                        fullWidth
                        margin="dense"
                        label="Reason"
                        value={reason}
                        onChange={(e) => setReason(e.target.value)}
                    />
                )}
            </DialogContent>
            <DialogActions>
                <Button
                    variant='outlined'
                    onClick={() => handleClose({ confirmed: false })}
                >
                    {options.cancelText ?? "Cancel"}
                </Button>
                <Button
                    onClick={() => handleClose({ confirmed: true, reason })}
                    variant="contained"
                    disabled={options.requireReason && !reason.trim()}
                >
                    {options.confirmText ?? "Confirm"}
                </Button>
            </DialogActions>
        </Dialog>
    ) : null;

    return { confirm, ConfirmDialogUI };
}