import { Box, Toolbar, useMediaQuery, useTheme } from "@mui/material";
import { useState } from "react";
import Header from "../components/layouts/Header";
import { Sidebar } from "../components/layouts/Sidebar";
import { Outlet } from "react-router-dom";
import { SignalRProvider } from "../app/provider/SignalRProvider";

function AppLayout() {
    const theme = useTheme();
    const isDesktop = useMediaQuery(theme.breakpoints.up('md'));

    const [mobileOpen, setMobileOpen] = useState(false);
    const [collapsed, setCollapsed] = useState(false);

    const handleMenuClick = () => {
        if (isDesktop) {
            setCollapsed(prev => !prev);
        } else {
            setMobileOpen(prev => !prev);
        }
    }

    return (
        <SignalRProvider>
            <Box sx={{ display: "flex" }}>
                <Header onMenuClick={handleMenuClick} />

                <Sidebar
                    variant={isDesktop ? "permanent" : "temporary"}
                    open={mobileOpen}
                    collapsed={collapsed}
                    onClose={() => setMobileOpen(false)}
                />

                <Box component="main" sx={{ flexGrow: 1, p: 0, minWidth: 0, minHeight: "100vh", bgcolor: "background.default" }}>
                    <Toolbar />
                    <Outlet />
                </Box>
            </Box>
        </SignalRProvider>
    );
}

export default AppLayout;