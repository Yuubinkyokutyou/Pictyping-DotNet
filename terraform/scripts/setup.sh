#!/bin/bash
set -e

# <UDF name="gh_token" label="GitHub Token" />
# <UDF name="repo_url" label="Repository URL" />
# <UDF name="branch" label="Branch" />
# <UDF name="environment" label="Environment" />

# Get StackScript parameters
GH_TOKEN="$GH_TOKEN"
REPO_URL="$REPO_URL" 
BRANCH="$BRANCH"
ENVIRONMENT="$ENVIRONMENT"

# Update system
apt-get update
apt-get upgrade -y

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh
systemctl enable docker
systemctl start docker

# Install Docker Compose
curl -L "https://github.com/docker/compose/releases/download/v2.24.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
chmod +x /usr/local/bin/docker-compose

# Install essential tools
apt-get install -y git nginx certbot python3-certbot-nginx

# Create app user
useradd -m -s /bin/bash pictyping
usermod -aG docker pictyping

# Mount volume
mkdir -p /mnt/data
echo "/dev/disk/by-id/scsi-0Linode_Volume_* /mnt/data ext4 defaults,noatime,nofail 0 2" >> /etc/fstab
mount -a

# Create data directories
mkdir -p /mnt/data/postgres
mkdir -p /mnt/data/redis
mkdir -p /mnt/data/letsencrypt
chown -R pictyping:pictyping /mnt/data

# Clone repository
cd /home/pictyping
sudo -u pictyping git clone -b $BRANCH $REPO_URL app
cd app/Pictyping-DotNet

# Create symbolic links for persistent data
ln -s /mnt/data/postgres postgres_data
ln -s /mnt/data/redis redis_data
ln -s /mnt/data/letsencrypt letsencrypt

# Create environment-specific docker-compose override
cat > docker-compose.${ENVIRONMENT}.yml <<EOF
version: '3.8'

services:
  api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5000
    restart: unless-stopped

  web:
    restart: unless-stopped

  nginx:
    restart: unless-stopped
    volumes:
      - /mnt/data/letsencrypt:/etc/letsencrypt:ro

  db:
    restart: unless-stopped
    volumes:
      - /mnt/data/postgres:/var/lib/postgresql/data

  redis:
    restart: unless-stopped
    volumes:
      - /mnt/data/redis:/data
EOF

# Create systemd service
cat > /etc/systemd/system/pictyping.service <<EOF
[Unit]
Description=Pictyping Application
Requires=docker.service
After=docker.service

[Service]
Type=oneshot
RemainAfterExit=yes
WorkingDirectory=/home/pictyping/app/Pictyping-DotNet
ExecStart=/usr/local/bin/docker-compose -f docker-compose.yml -f docker-compose.${ENVIRONMENT}.yml up -d
ExecStop=/usr/local/bin/docker-compose -f docker-compose.yml -f docker-compose.${ENVIRONMENT}.yml down
User=pictyping
Group=pictyping

[Install]
WantedBy=multi-user.target
EOF

# Enable and start service
systemctl daemon-reload
systemctl enable pictyping
systemctl start pictyping

# Wait for Docker containers to be built and started
echo "Waiting for Docker containers to build and start..."
sleep 30

# Check if containers are running
cd /home/pictyping/app/Pictyping-DotNet
for i in {1..10}; do
    if docker-compose ps | grep -q "Up"; then
        echo "Containers are starting up..."
        break
    fi
    echo "Waiting for containers... (attempt $i/10)"
    sleep 30
done

# Show final status
echo "Final container status:"
docker-compose ps

# Configure firewall
ufw allow 22/tcp
ufw allow 80/tcp
ufw allow 443/tcp
ufw --force enable

# Setup automatic updates
cat > /etc/apt/apt.conf.d/50unattended-upgrades <<EOF
Unattended-Upgrade::Allowed-Origins {
    "\${distro_id}:\${distro_codename}-security";
};
Unattended-Upgrade::AutoFixInterruptedDpkg "true";
Unattended-Upgrade::MinimalSteps "true";
Unattended-Upgrade::Remove-Unused-Dependencies "true";
Unattended-Upgrade::Automatic-Reboot "false";
EOF

systemctl enable unattended-upgrades
systemctl start unattended-upgrades

echo "Setup completed successfully!"