import { useSnackbar, type OptionsObject, type SnackbarKey, type SnackbarMessage } from "notistack";
import { useEffect } from "react";

let enqueueSnackbarRef: (message: SnackbarMessage, options?: OptionsObject) => SnackbarKey;

export const toast = {
    success: (message: string) => enqueueSnackbarRef(message, { variant: 'success' }),
    error: (message: string) => enqueueSnackbarRef(message, { variant: 'error' }),
    warning: (message: string) => enqueueSnackbarRef(message, { variant: 'warning' }),
    info: (message: string) => enqueueSnackbarRef(message, { variant: 'info' })
}

export function ToastConnector() {
    const { enqueueSnackbar } = useSnackbar();

    useEffect(() => {
        enqueueSnackbarRef = enqueueSnackbar
    }, [enqueueSnackbar]);

    return null;
}