import { createRoot } from 'react-dom/client'
import { Provider } from "react-redux";
import './index.css'
import App from './App.tsx'
import { BrowserRouter } from 'react-router-dom';
import store from './app/store.ts';
import { SnackbarProvider } from "notistack";

createRoot(document.getElementById('root')!).render(
  <Provider store={store}>
    <BrowserRouter>
      <SnackbarProvider maxSnack={3} anchorOrigin={{ vertical: 'top', horizontal: 'right'}} autoHideDuration={3000}>
        <App />
      </SnackbarProvider>
    </BrowserRouter>
  </Provider>,
)
