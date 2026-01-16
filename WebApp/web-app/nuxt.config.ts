import tailwindcss from '@tailwindcss/vite'
// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: false },
  ssr: false, // non-SSR
  runtimeConfig: {
    // apiSecret: 'server-only-value',        // hanya server
    public: {
      // baseUrl: 'http://localhost:65293/api'    //
      baseUrl: '/api'    //
    }
  },
  app: {
    head: {
      title: 'Starter'
    },
    pageTransition: { name: 'page', mode: 'out-in' },
    layoutTransition: { name: 'layout', mode: 'out-in' },
  
    // Page loading indicator
    spaLoaderAttrs: {
      class: 'my-loader',
      'data-test': 'loader',
      style: 'background-color: #29d; height: 4px;'
    }
  },
  modules: [
    '@nuxt/icon', 'shadcn-nuxt', '@pinia/nuxt'
  ],
  css: ['~/assets/css/tailwind.css', '~/assets/app.scss'],
  vite: {
    plugins: [
      tailwindcss(),
    ],
  },
  shadcn: {
    /**
     * Prefix for all the imported component
     */
    prefix: '',
    /**
     * Directory that the component lives in.
     * @default "./components/ui"
     */
    componentDir: './app/components/shadcn/components/ui',
  }
})