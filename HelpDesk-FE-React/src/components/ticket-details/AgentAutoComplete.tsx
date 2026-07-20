import { useEffect, useRef, useState } from "react";
import type { AgentOption } from "../../interfaces/user";
import { useDebouncedValue } from "../../app/hooks/debounced-hook";
import { getAssinableAgentsAsync } from "../../services/user-service";
import { toast } from "../../utils/Toast";
import { Autocomplete, Box, CircularProgress, TextField } from "@mui/material";

const PAGE_SIZE = 10;

interface Props {
    value: AgentOption | null;
    onChange: (agent: AgentOption | null) => void;
    disabled?: boolean;
}

const AgentAutocomplete = ({ value, onChange, disabled }: Props) => {
    const [open, setOpen] = useState(false);
    const [inputValue, setInputValue] = useState('');
    const [agents, setAgents] = useState<AgentOption[]>([]);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [loading, setLoading] = useState(false);

    const debouncedSearch = useDebouncedValue(inputValue, 400);
    const listboxRef = useRef<HTMLUListElement | null>(null);

    // Reset to page 1 whenever the search term changes (new search = fresh list)
    useEffect(() => {
        if (!open) return;
        fetchAgents(1, debouncedSearch, true);
    }, [debouncedSearch]);

    const fetchAgents = async (pageToFetch: number, search: string, replace: boolean) => {
        setLoading(true);
        try {
            const response = await getAssinableAgentsAsync({
                search: search || undefined,
                page: pageToFetch,
                pageSize: PAGE_SIZE,
            });
            const { items, totalPages: tp } = response.data;
            setAgents((prev) => (replace ? items : [...prev, ...items]));
            setTotalPages(tp);
            setPage(pageToFetch);
        } catch (error) {
            toast.error(error);
        } finally {
            setLoading(false);
        }
    };

    const handleScroll = (event: React.UIEvent<HTMLUListElement>) => {
        const listbox = event.currentTarget;
        const nearBottom = listbox.scrollTop + listbox.clientHeight >= listbox.scrollHeight - 40;
        if (nearBottom && !loading && page < totalPages) {
            fetchAgents(page + 1, debouncedSearch, false);
        }
    };

    return (
        <Autocomplete
            open={open}
            onOpen={() => {
                setOpen(true);
                if (agents.length === 0) fetchAgents(1, '', true);
            }}
            onClose={() => setOpen(false)}
            value={value}
            onChange={(_, newValue) => onChange(newValue)}
            inputValue={inputValue}
            onInputChange={(_, newInputValue, reason) => {
                if (reason === 'input' || reason === 'reset') setInputValue(newInputValue);
            }}
            options={agents}
            getOptionLabel={(agent) => agent.name}
            isOptionEqualToValue={(a, b) => a.id === b.id}
            loading={loading}
            disabled={disabled}
            filterOptions={(x) => x} // disable client-side filtering — backend already filtered
            slotProps={{
                listbox: {
                    onScroll: handleScroll,
                    ref: listboxRef,
                    style: { maxHeight: 280 },
                },
            }}
            renderInput={(params) => (
                <TextField
                    {...params}
                    label="Search agent"
                    placeholder="Type a name..."
                    slotProps={{
                        ...params.slotProps,
                        input: {
                            ...params.slotProps?.input,
                            endAdornment: (
                                <>
                                    {loading && <CircularProgress size={20} />}
                                    {params.slotProps?.input?.endAdornment}
                                </>
                            ),
                        }
                    }}
                />
            )}
            renderOption={(props, agent) => (
                <Box component="li" {...props} key={agent.id}>
                    {agent.name}
                </Box>
            )}
            noOptionsText={loading ? 'Loading...' : 'No agents found'}
        />
    );
};

export default AgentAutocomplete;