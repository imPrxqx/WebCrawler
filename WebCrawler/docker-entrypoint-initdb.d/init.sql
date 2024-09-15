CREATE TABLE "WebsiteRecord" (
    "Id" SERIAL PRIMARY KEY,
    "Url" VARCHAR(255) NOT NULL,
    "BoundaryRegExp" VARCHAR(255) NOT NULL,
    "LastChange" TIMESTAMP,
    "Days" INT NOT NULL,
    "Hours" INT NOT NULL,
    "Minutes" INT NOT NULL,
    "Label" VARCHAR(255) NOT NULL,
    "IsActive" BOOLEAN NOT NULL,
    "Tags" VARCHAR(255),
    "LastExecution" TIMESTAMP,
    "LastStatus" BOOLEAN       
);

CREATE TABLE "Node" (
    "Id" SERIAL PRIMARY KEY,
    "Title" VARCHAR(255) NOT NULL,
    "CrawlTime" VARCHAR(255) NOT NULL,
    "UrlMain" VARCHAR(255) NOT NULL,
    "WebsiteRecordId" INT,
    FOREIGN KEY ("WebsiteRecordId") REFERENCES "WebsiteRecord"("Id") 
);

CREATE TABLE "NodeNeighbour" (
    "NodeId" INT,
    "NeighbourNodeId" INT,
    PRIMARY KEY ("NodeId", "NeighbourNodeId"),
    FOREIGN KEY ("NodeId") REFERENCES "Node"("Id"),
    FOREIGN KEY ("NeighbourNodeId") REFERENCES "Node"("Id")
)
