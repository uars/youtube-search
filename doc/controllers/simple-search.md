# Simple Search

```csharp
SimpleSearchController simpleSearchController = client.SimpleSearchController;
```

## Class Name

`SimpleSearchController`


# Search by Keyword

```csharp
SearchByKeywordAsync(
    Models.TypeSearchEnum search,
    string part,
    int maxResults,
    string key,
    string q)
```

## Parameters

| Parameter | Type | Tags | Description |
|  --- | --- | --- | --- |
| `search` | [`Models.TypeSearchEnum`](/doc/models/type-search-enum.md) | Template, Required | - |
| `part` | `string` | Query, Required | - |
| `maxResults` | `int` | Query, Required | **Constraints**: `>= 10`, `<= 50`, *Multiple Of*: `5`, *Total Digits*: `2` |
| `key` | `string` | Query, Required | API-Key |
| `q` | `string` | Query, Required | keyword<br>**Constraints**: *Minimum Length*: `5`, *Maximum Length*: `10`, *Pattern*: `A-Z` |

## Response Type

`Task<dynamic>`

## Example Usage

```csharp
TypeSearchEnum search = TypeSearchEnum.Search;
string part = "snippet";
int maxResults = 25;
string key = "AIzaSyAzYmRVV7HvVqh6OcNgbB4AC8NcjyXJBR4";
string q = "q0";

try
{
    dynamic result = await simpleSearchController.SearchByKeywordAsync(search, part, maxResults, key, q);
}
catch (ApiException e){};
```

## Errors

| HTTP Status Code | Error Description | Exception Class |
|  --- | --- | --- |
| 400 | The parameter specified has an invalid argument | [`InvalidArgumentException`](/doc/models/invalid-argument-exception.md) |
| 402 | The relatedToVideo parameter specified an invalid video ID | [`InvalidVideoIdException`](/doc/models/invalid-video-id-exception.md) |

