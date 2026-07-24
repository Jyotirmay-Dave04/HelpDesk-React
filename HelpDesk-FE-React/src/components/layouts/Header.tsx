import { useAppDispatch, useAppSelector } from "../../app/hooks/hooks";
import { useState } from "react";
import { logout } from "../../app/slices/auth-slice";
import { AppBar, Avatar, Box, Divider, IconButton, Menu, MenuItem, Stack, Toolbar, Typography } from "@mui/material";
import { Menu as MenuIcon } from "@mui/icons-material";
import { useNavigate } from "react-router-dom";
import NotificationBell from "../header/NotificationBell";

interface HeaderProps {
    onMenuClick: () => void;
}

function Header({ onMenuClick }: HeaderProps) {
    const { user } = useAppSelector(state => state.auth);
    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

    const handleLogout = () => {
        dispatch(logout());
        navigate('/login', { replace: true });
    }

    const initials = user?.name?.split(" ").map(p => p[0]).slice(0, 2).join("").toUpperCase();

    return (
        <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
            <Toolbar>
                <IconButton color="inherit" edge="start" onClick={onMenuClick} sx={{ mr: 2 }}>
                    <MenuIcon />
                </IconButton>
                <Typography variant="h6" noWrap sx={{ flexGrow: 1 }}>
                    HelpDesk
                </Typography>
                <Box>
                    <Stack direction='row' spacing={3}>
                        <NotificationBell />
                        <IconButton onClick={(e) => setAnchorEl(e.currentTarget)}>
                            <Avatar sx={{ width: 32, height: 32, bgcolor: "secondary.main" }}>{initials}</Avatar>
                        </IconButton>
                        <Menu anchorEl={anchorEl} open={!!anchorEl} onClose={() => setAnchorEl(null)}>
                            <MenuItem disabled>{user?.name}</MenuItem>
                            <Divider />
                            <MenuItem onClick={handleLogout}>Logout</MenuItem>
                        </Menu>
                    </Stack>
                </Box>
            </Toolbar>
        </AppBar>
    );
}

export default Header;