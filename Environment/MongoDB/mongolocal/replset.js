rsconf = {
  _id : 'rs0',
  members: [
    { _id : 0, host : 'mongo1:28017' },
    { _id : 1, host : 'mongo2:28018' },
    { _id : 2, host : 'mongo3:28019' }
  ]
}

rsconf.members[0].priority = 1
rsconf.members[1].priority = 0.5
rsconf.members[2].priority = 0.5

rs.initiate(rsconf);
rs.conf();
