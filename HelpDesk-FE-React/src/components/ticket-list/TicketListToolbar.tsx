import { FilterList, Search } from "@mui/icons-material";
import { Badge, Box, Button, InputAdornment, Stack, TextField } from "@mui/material";

interface Props {
    searchValue: string;
    onSearchChange: (value: string) => void;
    activeFilterCount: number;
    onOpenFilterPanel: () => void;
}

const TicketListToolbar = ({ searchValue, onSearchChange, activeFilterCount, onOpenFilterPanel }: Props) => {
    return (
        <Stack direction="row" spacing={2} sx={{ mb: 2, alignItems: 'center' }}>
            <TextField
                placeholder="Search tickets..."
                size="small"
                value={searchValue}
                onChange={(e) => onSearchChange(e.target.value)}
                sx={{ minWidth: 320 }}
                slotProps={{
                    input: {
                        startAdornment: (
                            <InputAdornment position="start">
                                <Search fontSize="small" />
                            </InputAdornment>
                        ),
                    },
                }}
            />
            <Box sx={{ flexGrow: 1 }} />
            <Badge badgeContent={activeFilterCount} color="primary">
                <Button variant="outlined" startIcon={<FilterList />} onClick={onOpenFilterPanel}>
                    Filters
                </Button>
            </Badge>
        </Stack>
    );
};

export default TicketListToolbar;