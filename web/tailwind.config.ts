import type { Config } from 'tailwindcss'

export default {
  content: [
    './components/**/*.{vue,js,ts}',
    './layouts/**/*.vue',
    './pages/**/*.vue',
    './composables/**/*.{js,ts}',
    './plugins/**/*.{js,ts}',
    './app.vue'
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Inter', 'Helvetica', 'sans-serif'],
        mono: ['"JetBrains Mono"', 'ui-monospace', 'SFMono-Regular', 'monospace']
      },
      colors: {
        // Dark surfaces (kept)
        bg: '#0a0a0c',
        surface: '#141418',
        elevated: '#1c1c22',
        border: '#26262e',
        muted: '#71717a',
        text: '#f4f4f5',

        // Design.com primary indigo — brand color for solid surfaces (buttons, spine)
        accent: '#3f59f6',
        // Lighter indigo (dcom primary-400) — used for text + icons on dark surfaces
        // where the brand 500 doesn't have enough luminance to read at a glance
        'accent-soft': '#798bf9',

        // Design.com semantic colors
        warning: '#E3A008',
        danger: '#E34935',
        success: '#22A06B'
      },
      keyframes: {
        'fade-up': {
          '0%': { opacity: '0', transform: 'translateY(8px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' }
        },
        'slide-in-right': {
          '0%': { opacity: '0', transform: 'translateX(24px)' },
          '100%': { opacity: '1', transform: 'translateX(0)' }
        },
        'fade-in': {
          '0%': { opacity: '0' },
          '100%': { opacity: '1' }
        },
        'rollout-grow': {
          '0%': { transform: 'scaleX(0)' },
          '100%': { transform: 'scaleX(1)' }
        }
      },
      animation: {
        'fade-up': 'fade-up 400ms cubic-bezier(0.16, 1, 0.3, 1) both',
        'slide-in-right': 'slide-in-right 250ms cubic-bezier(0.4, 0, 0.2, 1) both',
        'fade-in': 'fade-in 200ms ease-out both',
        'rollout-grow': 'rollout-grow 600ms cubic-bezier(0.16, 1, 0.3, 1) both'
      }
    }
  },
  plugins: []
} satisfies Config
