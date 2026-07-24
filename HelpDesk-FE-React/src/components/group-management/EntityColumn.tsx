import { Add, Edit, Delete } from "@mui/icons-material";
import { Paper, Stack, Typography, Tooltip, IconButton, Box, CircularProgress, List, ListItemButton, ListItemText, Pagination } from "@mui/material";

interface ColumnEntity {
    id: number;
    name: string;
}

interface EntityColumnProps<T extends ColumnEntity> {
    title: string;
    items: T[];
    selectedId: number | null;
    loading: boolean;
    disabled?: boolean;
    disabledMessage?: string;
    page: number;
    totalPages: number;
    onPageChange: (page: number) => void;
    onSelect: (id: number) => void;
    onAdd: () => void;
    onEdit: (item: T) => void;
    onDelete: (item: T) => void;
}

function EntityColumn<T extends ColumnEntity>({
    title,
    items,
    selectedId,
    loading,
    disabled,
    disabledMessage,
    page,
    totalPages,
    onPageChange,
    onSelect,
    onAdd,
    onEdit,
    onDelete,
}: EntityColumnProps<T>) {
    return (
        <Paper variant="outlined" sx={{ flex: 1, minWidth: 280, display: "flex", flexDirection: "column", height: 600 }}>
            <Stack direction="row" sx={{ px: 2, py: 1.5, borderBottom: "1px solid", borderColor: "divider", alignItems: "center", justifyContent: "space-between" }}>
                <Typography variant="subtitle1" sx={{fontWeight: 600}}>{title}</Typography>
                <Tooltip title={disabled ? (disabledMessage ?? "") : `Add ${title.replace(/s$/, "")}`}>
                    <span>
                        <IconButton size="small" color="primary" onClick={onAdd} disabled={disabled}>
                            <Add fontSize="small" />
                        </IconButton>
                    </span>
                </Tooltip>
            </Stack>

            <Box sx={{ flex: 1, overflowY: "auto" }}>
                {loading ? (
                    <Stack sx={{ height: "100%", alignItems: 'center', justifyContent: 'center' }}>
                        <CircularProgress size={28} />
                    </Stack>
                ) : disabled ? (
                    <Typography variant="body2" color="text.secondary" sx={{ p: 2 }}>
                        {disabledMessage}
                    </Typography>
                ) : items.length === 0 ? (
                    <Typography variant="body2" color="text.secondary" sx={{ p: 2 }}>
                        No {title.toLowerCase()} yet.
                    </Typography>
                ) : (
                    <List disablePadding>
                        {items.map((item) => (
                            <ListItemButton
                                key={item.id}
                                selected={item.id === selectedId}
                                onClick={() => onSelect(item.id)}
                                sx={{
                                    borderLeft: 3,
                                    borderColor: item.id === selectedId ? "primary.main" : "transparent",
                                }}
                            >
                                <ListItemText primary={item.name} />
                                <Stack direction="row" onClick={(e) => e.stopPropagation()}>
                                    <Tooltip title="Edit">
                                        <IconButton size="small" color='primary' onClick={() => onEdit(item)}>
                                            <Edit fontSize="small" />
                                        </IconButton>
                                    </Tooltip>
                                    <Tooltip title="Delete">
                                        <IconButton size="small" color="error" onClick={() => onDelete(item)}>
                                            <Delete fontSize="small" />
                                        </IconButton>
                                    </Tooltip>
                                </Stack>
                            </ListItemButton>
                        ))}
                    </List>
                )}
            </Box>

            {!disabled && totalPages > 1 && (
                <Stack sx={{ py: 1, borderTop: "1px solid", borderColor: "divider", alignItems: 'center' }}>
                    <Pagination
                        size="small"
                        count={totalPages}
                        page={page}
                        onChange={(_, value) => onPageChange(value)}
                        disabled={loading}
                    />
                </Stack>
            )}
        </Paper>
    );
}

export default EntityColumn;