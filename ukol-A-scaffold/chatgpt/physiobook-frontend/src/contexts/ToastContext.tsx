import { createContext, useContext, useMemo, useState } from "react";

export interface ToastItem {
  id: string;
  title: string;
  kind?: "success" | "error" | "info";
}

interface ToastContextValue {
  showToast: (title: string, kind?: ToastItem["kind"]) => void;
}

const ToastContext = createContext<ToastContextValue | undefined>(undefined);

export function ToastProvider({ children }: { children: React.ReactNode }) {
  const [toasts, setToasts] = useState<ToastItem[]>([]);

  const showToast = (title: string, kind: ToastItem["kind"] = "info") => {
    const id = crypto.randomUUID();
    setToasts((current) => [...current, { id, title, kind }]);

    window.setTimeout(() => {
      setToasts((current) => current.filter((toast) => toast.id !== id));
    }, 3500);
  };

  const value = useMemo(() => ({ showToast }), []);

  return (
    <ToastContext.Provider value={value}>
      {children}
      <div className="fixed right-4 top-4 z-50 flex w-80 flex-col gap-3">
        {toasts.map((toast) => (
          <div
            key={toast.id}
            className="rounded-2xl border border-slate-200 bg-white px-4 py-3 shadow-lg"
          >
            <p className="text-sm font-medium">{toast.title}</p>
            <p className="mt-1 text-xs text-slate-500">{toast.kind ?? "info"}</p>
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  );
}

export function useToast(): ToastContextValue {
  const context = useContext(ToastContext);

  if (!context) {
    throw new Error("useToast must be used within ToastProvider.");
  }

  return context;
}
