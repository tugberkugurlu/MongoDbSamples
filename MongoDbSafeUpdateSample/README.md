## The Problem

In MongoDB world, you cannot issue cross collection queries. This's a problem when you need to reference a document 
in a collection from a document inside another collection. Following sample demonstrates the problem:

A "Category" document:

    {
        "_id": 1,
        "name": "Electronics",
        "lastUpdatedOn": ISODate("2014-05-29T09:04:07.409Z"),
        "images": [
            {
                "path": "http://example.com/images/1_1.jpg",
                "order": 2
            },
            {
                "path": "http://example.com/images/1_3.jpg",
                "order": 3
            },
            {
                "path": "http://example.com/images/1_25.jpg",
                "order": 1
            }
        ]
    }

A "Product" document:

    {
        "_id": 100,
        "categoryId": "1",
        "name": "MacBook Pro",
        "lastUpdatedOn": ISODate("2014-05-29T12:04:02.789Z")

        // so on and so forth
    }

The problem here is that when you query for a product, you cannot get the category name of a product. There 
are a few solutions to this problem.

## 1-) Embed the Category and Update Each Product Document When a Category is Updated

TODO: Write it down

## Questions:

 - What if an update takes long time?
 - What if an update to a product document fails during the update process?

## Message Queueing System

### Rules and Things to Consider

 - When an update or delete requested, try to stick a queue message immediately.
 - However, a queue item is not quaranteed to be queued after the update operation is completed because of lots of reasons.
 - So, we need to be able to detect that a document is updated/deleted and successfully queued to be processed. 
   With this information in place, we can now have a worker process to watch the collection for unhealthy updates 
   (unhealthy means that updated but could not be queued immediately) and create queue items for that.
 - 