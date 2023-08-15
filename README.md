# Beer Web Api
This application is generating a .NET Core Web Api which stores, updates and retrieves different types of beer, their associated breweries and bars that serve the beers.

## To Run this application
* Download the source code from the GitHub Page.
* Configure the DB Path in the appsettings.json file in VS solution 
  "ConnectionStrings": { "DefaultConnection":"Data Source = {Local Path}\\Source\\BeerWeb.Api.DataAccess\\Database\\BeerStoreDb.db" }
* Once the above configuration is done you are good to run API with the Swagger.
  
## Data Model
### Beer
| Property                  | Type    |
| ------------------------- | ------- |
| Name                      | string  |
| PercentageAlcoholByVolume | decimal |

### Brewery
| Property                  | Type    |
| ------------------------- | ------- |
| Name                      | string  |

### Bar
| Property                  | Type    |
| ------------------------- | ------- |
| Name                      | string  |
| Address                   | string  |

## Data Model Relationships
* Breweries can have many beers
* Bars can serve many types of beers

### BarBeer
| Property                  | Type    |
| ------------------------- | ------- |
| BarID                     | Number  |
| BeerId                    | Number  |

### BreweryBeer
| Property                  | Type    |
| ------------------------- | ------- |
| BreweryId                 | Number  |
| BeerId                    | Number  |

## Technical Stacks 

| Technical Stacks                  |
| ------------------------- |
| Web API Core 6.0 | 
| EF 7.0.9 Code First Approach | 
| SQLite Database | 
| Serilog Logger |
| Dependency Injection | 
| AutoMapper|
| Linq | 
| UnitOfWork |
| Swagger - Validate API | 
| Unit Tests | 

## Swagger samples 

The samples for posting and updating the related endpoints are provided below.
### Beer - Post & Put
```bash
{							
  "beerId": 100,
  "name": "Oatmeal Stouts",
  "percentageAlcoholByVolume": 5.0				
}	
```
### Brewery - Post & Put
```bash
{  
  "breweryId": 25,
  "name": "Upslope Brewing Company"
}
```
### Brewery Beer - Post
```bash
{  
  "id": 10,
  "breweryId": 25,
  "beerId": 100
}
```

### Bar - Post & Put
```bash
{
  "barId": 5,
  "name": "Paradise: After Dark",
  "address": "61 Rupert St, London W1D 7PW"
}
```
### Bar Beer - Post
```bash
{  
  "id": 12,
  "barId": 5,
  "beerId": 100
}
```
