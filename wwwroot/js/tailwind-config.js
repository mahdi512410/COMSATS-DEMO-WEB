// Single source of truth for the portal's design tokens (the "Collegiate
// Precision" system). Every view loads this file once through _Layout.cshtml
// instead of repeating the token object inline.
tailwind.config = {
  theme: {
    extend: {
      colors: {
        background: "#f8f9fb",
        surface: "#ffffff",
        "surface-container-lowest": "#ffffff",
        "surface-container-low": "#f8f9fa",
        "surface-container": "#f1f3f5",
        "surface-container-high": "#e5e7eb",
        "on-surface": "#111827",
        "on-surface-variant": "#4b5563",
        "on-surface-muted": "#9ca3af",
        outline: "#e5e7eb",
        primary: "#6b1226",
        "primary-deep": "#4a0d1a",
        "on-primary": "#ffffff",
        gold: "#d4af37",
        "gold-deep": "#c59b27",
        "gold-tint": "#f7f1e1",
        success: "#047857",
        "success-surface": "#ecfdf5",
        "success-border": "#a7f3d0",
        warning: "#b45309",
        "warning-surface": "#fffbeb",
        "warning-border": "#fde68a",
        critical: "#b91c1c",
        "critical-surface": "#fef2f2",
        "critical-border": "#fecaca",
      },
      fontFamily: {
        sans: ["Inter", "system-ui", "sans-serif"],
      },
      borderRadius: {
        sm: "2px",
        DEFAULT: "4px",
        md: "6px",
        lg: "8px",
        xl: "12px",
      },
      boxShadow: {
        card: "0 1px 2px rgba(17, 24, 39, 0.04)",
        raised: "0 4px 12px rgba(17, 24, 39, 0.06), 0 1px 2px rgba(17, 24, 39, 0.04)",
        modal: "0 20px 25px -5px rgba(17, 24, 39, 0.1), 0 8px 10px -6px rgba(17, 24, 39, 0.05)",
      },
      spacing: {
        18: "4.5rem",
      },
    },
  },
};
