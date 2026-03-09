CREATE KEYSPACE IF NOT EXISTS tripex
WITH replication = {
  'class': 'SimpleStrategy',
  'replication_factor': 1
};

USE tripex;

CREATE TABLE IF NOT EXISTS posts_by_id (
  id uuid PRIMARY KEY,
  profile_id uuid,
  content_url text,
  description text,
  created_at timestamp
);

CREATE TABLE IF NOT EXISTS posts_by_author (
  profile_id uuid,
  id uuid,
  content_url text,
  description text,
  created_at timestamp,
  PRIMARY KEY (profile_id, id)
) WITH CLUSTERING ORDER BY (id DESC);

