# vacd.api
**Deploying IaC through GitHub Actions - API:** This repository contains the api source code written in C# in order to create Resource Groups in the bastioned subscription.

### Azure resources to be deployed:
- WebApp Code (build .NET + release)

### SPNs used:
- Contributor SPN with rights under CommunityDayRSG
- Master SPN with rights under bastioned subscription

