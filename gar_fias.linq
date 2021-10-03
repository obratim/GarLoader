<Query Kind="Statements">
  <NuGetReference>SqlWorker.MySql</NuGetReference>
  <RuntimeVersion>5.0</RuntimeVersion>
</Query>

using var sw = new SqlWorker.MySqlWorker("Server=127.0.0.1;Port=3307;Database=gar;Uid=gar_user;Pwd=1234");

sw.Exec(@"DROP TABLE municipal_hierarchy");
sw.Exec(@"DROP TABLE administrative_hierarchy");
sw.Exec(@"DROP TABLE address_object_param");
sw.Exec(@"DROP TABLE address_object");

sw.Exec(@"DROP TABLE address_object_type");
sw.Exec(@"DROP TABLE object_level");
sw.Exec(@"DROP TABLE operation_type");
sw.Exec(@"DROP TABLE param_type");

sw.Exec(@"
CREATE TABLE address_object_type (
	id INTEGER NOT NULL PRIMARY KEY,
	level INTEGER NOT NULL,
	shortname TEXT NOT NULL,
	name TEXT NOT NULL,
	description TEXT NULL,
	update_date DATE NOT NULL,
	start_date DATE NOT NULL,
	end_date DATE NOT NULL
	-- is_active BOOL NOT NULL
);
");


sw.Exec(@"
CREATE TABLE object_level (
	level INTEGER NOT NULL PRIMARY KEY,
	name VARCHAR(250) NOT NULL,
	shortname VARCHAR(50) NULL,
	update_date DATE NOT NULL,
	start_date DATE NOT NULL,
	end_date DATE NOT NULL
	-- is_active BOOL NOT NULL
);
");

sw.Exec(@"
CREATE TABLE operation_type (
	id INTEGER NOT NULL PRIMARY KEY,
	name VARCHAR(100) NOT NULL,
	shortname VARCHAR(100) NULL,
	description VARCHAR(250) NULL,
	update_date DATE NOT NULL,
	start_date DATE NOT NULL,
	end_date DATE NOT NULL
	-- is_active BOOL NOT NULL
);
");

sw.Exec(@"
CREATE TABLE param_type (
	id INTEGER NOT NULL PRIMARY KEY,
	name VARCHAR(50) NOT NULL,
	code VARCHAR(50) NOT NULL,
	description VARCHAR(120) NULL,
	update_date DATE NOT NULL,
	start_date DATE NOT NULL,
	end_date DATE NOT NULL
	-- is_active BOOL NOT NULL
);
");

sw.Exec(@"
CREATE TABLE address_object (
	id BIGINT NOT NULL,
	object_id BIGINT NOT NULL, -- PRIMARY KEY,
	object_guid CHAR(36) NOT NULL,
	change_id BIGINT NOT NULL,
	name VARCHAR(250) NOT NULL,
	region int null,
	type_name VARCHAR(50) NOT NULL,
	level INTEGER NOT NULL REFERENCES object_level (level),
	operation_type_id INT NOT NULL REFERENCES operation_type (id),
	prev_id BIGINT NULL,
	next_id BIGINT NULL,
	update_date DATE NOT NULL,
	start_date DATE NOT NULL,
	end_date DATE NOT NULL,
	is_active TINYINT NOT NULL,
	is_actual TINYINT NOT NULL
);
");

sw.Exec(@"
CREATE INDEX IX_address_object_object_id
ON address_object (object_id)
");

sw.Exec(@"
CREATE TABLE address_object_param (
	id BIGINT NOT NULL PRIMARY KEY,
	object_id BIGINT NOT NULL REFERENCES address_object (object_id),
	change_id BIGINT NULL,
	change_id_end BIGINT NOT NULL,
	type_id INT NOT NULL REFERENCES param_type (id),
	value VARCHAR(8000) NOT NULL,
	update_date DATE NOT NULL,
	start_date DATE NOT NULL,
	end_date DATE NOT NULL
);
");

sw.Exec(@"
CREATE TABLE administrative_hierarchy (
	id BIGINT NOT NULL PRIMARY KEY,
	object_id BIGINT NOT NULL , -- REFERENCES  address_object (object_id),
	parent_object_id BIGINT NULL , -- REFERENCES  address_object (object_id),
	change_id BIGINT NOT NULL,
	region_code INT NULL,
	area_code INT NULL,
	city_code INT NULL,
	place_code INT NULL,
	plan_code INT NULL,
	street_code INT NULL,
	prev_id BIGINT NULL,
	next_id BIGINT NULL,
	update_date DATE NOT NULL,
	start_date DATE NOT NULL,
	end_date DATE NOT NULL
	-- is_active BOOL NOT NULL
);
");

sw.Exec(@"
CREATE TABLE municipal_hierarchy (
	id BIGINT NOT NULL PRIMARY KEY,
	object_id BIGINT NOT NULL , -- REFERENCES  address_object (object_id),
	parent_object_id BIGINT NULL , -- REFERENCES  address_object (object_id),
	change_id BIGINT NOT NULL,
	oktmo VARCHAR(11) NULL,
	prev_id BIGINT NULL,
	next_id BIGINT NULL,
	update_date DATE NOT NULL,
	start_date DATE NOT NULL,
	end_date DATE NOT NULL
	-- is_active INT NOT NULL
);
");

//sw.Exec(@"
//INSERT INTO object_level (level, name, shortname, update_date, start_date, end_date) VALUES
//(15, '<?unknown?>', NULL, '2021-09-09', '2021-09-09', '3999-01-01')
//");


"finished".Dump();
