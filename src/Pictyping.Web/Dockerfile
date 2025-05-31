# Development stage
FROM node:18-alpine AS development
WORKDIR /app

# Copy package files
COPY src/Pictyping.Web/package*.json ./

# Install dependencies
RUN npm install

# Copy application files
COPY src/Pictyping.Web/ .

# Expose port
EXPOSE 3000

# Start development server
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]

# Production build stage
FROM node:18-alpine AS build
WORKDIR /app

# Copy package files
COPY src/Pictyping.Web/package*.json ./

# Install dependencies
RUN npm install

# Copy application files
COPY src/Pictyping.Web/ .

# Build the application
RUN npm run build

# Production stage
FROM nginx:alpine AS production
# Copy built files
COPY --from=build /app/dist /usr/share/nginx/html
# Copy nginx configuration
COPY nginx/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]