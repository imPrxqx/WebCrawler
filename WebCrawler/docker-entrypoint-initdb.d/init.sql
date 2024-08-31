CREATE TABLE "WebsiteRecord" (
    "Id" SERIAL PRIMARY KEY,
    "Url" VARCHAR(255) NOT NULL,
    "BoundaryRegExp" VARCHAR(255) NOT NULL,
    "Days" INT NOT NULL,
    "Hours" INT NOT NULL,
    "Minutes" INT NOT NULL,
    "Label" VARCHAR(255) NOT NULL,
    "IsActive" BOOLEAN NOT NULL,
    "Tags" VARCHAR(255) NOT NULL
);
