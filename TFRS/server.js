const express = require('express');
const cors = require('cors');
const ADODB = require('node-adodb');

const app = express();
app.use(cors()); 
app.use(express.json());

const dbPath = 'C:\jap\backend\.accdb';


const connectionString = `Provider=Microsoft.ACE.OLEDB.12.0;Data Source=${dbPath};Persist Security Info=False;`;
const db = ADODB.open(connectionString);


app.get('/api/franchise/:id', async (req, res) => {
    const franchiseId = req.params.id;

    
    const sql = `SELECT * FROM MAINTABLE WHERE [TRICYCLE FRANCHISE NUMBER] = '${franchiseId}'`;

    try {
        const data = await db.query(sql);
        if (data.length === 0) {
            return res.status(404).json({ message: "Franchise record not found" });
        }
        res.json(data[0]); 
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: "Database query failed", details: error.message });
    }
});


app.listen(3000, () => {
    console.log('Backend server running safely on http:localhost:3000');
});
