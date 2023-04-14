# Introduction

This repository contains a Sample C# implementation for the BMW Group - Tech Talent Days hackathon.

# Getting Started

This service uses a kafka compact topics to store sample requests. Setting up and hosting a kafka instance can be done with the following steps:

Create the kafka using docker by running this command

```shell script
docker-compose up kafka
```

If you want to view your topics run the following command and navigate to `localhost:8070`

```shell script
docker-compose up akhq
```

Create a local SQL docker instance

```shell script
docker-compose up sql
```

You should now be able to run the service and connect to your kafka instance.
