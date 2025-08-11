import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  
  // Docker環境の場合はコンテナ名を使用
  const apiUrl = process.env.VITE_API_URL || env.VITE_API_URL || 'http://localhost:5000'
  const isDocker = apiUrl.includes('api:')
  const proxyTarget = isDocker ? 'http://api:5000' : apiUrl
  
  return {
    plugins: [react()],
    resolve: {
      alias: {
        '@': path.resolve(__dirname, './src'),
      },
    },
    server: {
      host: '0.0.0.0',
      port: 3000,
      proxy: {
        '/api': {
          target: proxyTarget,
          changeOrigin: true,
        },
      },
    },
  }
})