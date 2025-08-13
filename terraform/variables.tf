variable "linode_token" {
  description = "Linode API token"
  type        = string
  sensitive   = true
}

variable "project_name" {
  description = "Project name"
  type        = string
  default     = "pictyping"
}

variable "environment" {
  description = "Environment name (e.g., sandbox, staging, production)"
  type        = string
}

variable "linode_region" {
  description = "Linode region"
  type        = string
  default     = "ap-northeast" # Tokyo
}

variable "linode_type" {
  description = "Linode instance type"
  type        = string
  default     = "g6-nanode-1" # $5/month plan
}

variable "linode_image" {
  description = "Linode image"
  type        = string
  default     = "linode/ubuntu22.04"
}

variable "ssh_public_key" {
  description = "SSH public key for instance access"
  type        = string
}

variable "volume_size" {
  description = "Size of the data volume in GB"
  type        = number
  default     = 10
}

variable "github_token" {
  description = "GitHub personal access token for private repo access"
  type        = string
  sensitive   = true
}

variable "repository_url" {
  description = "Git repository URL"
  type        = string
  default     = "https://github.com/Yuubinkyokutyou/Pictyping.git"
}

variable "branch" {
  description = "Git branch to deploy"
  type        = string
  default     = "main"
}

# Application-specific variables
variable "aspnetcore_environment" {
  description = "ASP.NET Core environment"
  type        = string
  default     = "Production"
}

variable "jwt_key" {
  description = "JWT signing key"
  type        = string
  sensitive   = true
}

variable "google_client_id" {
  description = "Google OAuth client ID"
  type        = string
  sensitive   = true
}

variable "google_client_secret" {
  description = "Google OAuth client secret"
  type        = string
  sensitive   = true
}

variable "db_password" {
  description = "PostgreSQL password"
  type        = string
  sensitive   = true
}

variable "redis_password" {
  description = "Redis password"
  type        = string
  sensitive   = true
  default     = ""
}