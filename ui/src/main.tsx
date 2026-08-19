import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import "flatpickr/dist/flatpickr.css";
import i18n from "./i18n/config";
import { I18nextProvider } from "react-i18next";
import App from "./App.tsx";
import { AppWrapper } from "./components/common/PageMeta.tsx";
import { ThemeProvider } from "./context/ThemeContext.tsx";
import { AuthProvider } from "./context/AuthContext.tsx";
import { CompanyProvider } from "./context/CompanyContext.tsx";
import { PermissionProvider } from "./context/PermissionContext.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <I18nextProvider i18n={i18n}>
      <ThemeProvider>
        <AuthProvider>
          <CompanyProvider>
            <PermissionProvider>
              <AppWrapper>
                <App />
              </AppWrapper>
            </PermissionProvider>
          </CompanyProvider>
        </AuthProvider>
      </ThemeProvider>
    </I18nextProvider>
  </StrictMode>,
);
