terraform {
  required_providers {
    linode = {
      source  = "linode/linode"
      version = "~> 2.0"
    }
  }
  required_version = ">= 1.0"
}

provider "linode" {
  token = var.linode_token
}

# SSH Key
resource "linode_sshkey" "main" {
  label   = "${var.project_name}-${var.environment}-key"
  ssh_key = var.ssh_public_key
}

# Firewall
resource "linode_firewall" "main" {
  label = "${var.project_name}-${var.environment}-firewall"

  inbound {
    label    = "allow-ssh"
    action   = "ACCEPT"
    protocol = "TCP"
    ports    = "22"
    ipv4     = ["0.0.0.0/0"]
  }

  inbound {
    label    = "allow-http"
    action   = "ACCEPT"
    protocol = "TCP"
    ports    = "80"
    ipv4     = ["0.0.0.0/0"]
  }

  inbound {
    label    = "allow-https"
    action   = "ACCEPT"
    protocol = "TCP"
    ports    = "443"
    ipv4     = ["0.0.0.0/0"]
  }

  inbound_policy  = "DROP"
  outbound_policy = "ACCEPT"

  linodes = [linode_instance.main.id]
}

# Main Instance
resource "linode_instance" "main" {
  label           = "${var.project_name}-${var.environment}"
  image           = var.linode_image
  region          = var.linode_region
  type            = var.linode_type
  authorized_keys = [linode_sshkey.main.ssh_key]

  # StackScript for initial setup
  stackscript_id = linode_stackscript.docker_setup.id
  stackscript_data = {
    "gh_token"    = var.github_token
    "repo_url"    = var.repository_url
    "branch"      = var.branch
    "environment" = var.environment
  }

  tags = [var.project_name, var.environment]
}

# Volume for persistent data
resource "linode_volume" "data" {
  label  = "${var.project_name}-${var.environment}-data"
  region = var.linode_region
  size   = var.volume_size

  linode_id = linode_instance.main.id
}

# StackScript for Docker and App Setup
resource "linode_stackscript" "docker_setup" {
  label       = "${var.project_name}-${var.environment}-setup"
  description = "Setup Docker and deploy Pictyping"
  script      = file("${path.module}/scripts/setup.sh")
  images      = [var.linode_image]
}

# Outputs
output "instance_ip" {
  value = tolist(linode_instance.main.ipv4)[0]
}

output "instance_id" {
  value = linode_instance.main.id
}

output "volume_id" {
  value = linode_volume.data.id
}