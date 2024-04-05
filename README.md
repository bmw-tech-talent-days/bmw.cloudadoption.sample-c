# Introduction

This repository contains a Sample C# implementation for the BMW Group - Tech Talent Days hackathon.

# Getting Started

This service uses sql akhq and kafka compact topics to store sample requests. Setting up and hosting a kafka instance can be done with the following steps:

Create the kafka using docker by running this command

```shell script
docker-compose up -d kafka akhq sql
```

If you want to view your topics navigate to `localhost:8070`
