import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '30s', target: 20 },
        // { duration: '1m30s', target: 10 },
        // { duration: '20s', target: 0 },
    ],
    thresholds: {
        http_req_failed: ['rate<1'],
        http_req_duration: ['p(95)<20'],
        http_req_duration: ['p(100)<30'],
    },
};

const url = 'http://localhost:5165/user';


const params = {
    headers: {
        'Content-Type': 'application/json',
    },
};

export default function () {
    const name = 'Fabio' + Math.random();
    const payloadAdd = JSON.stringify({ name: name });
    const requestAdd = http.post(url, payloadAdd, params);
    const requestGetAll = http.get(url);
    const requestGetUser = http.get(`${url}/${name}`);
    const requestDelete = http.del(`${url}/${name}`);

    check(requestAdd, { 'status was 201': (r) => r.status == 201 });
    check(requestGetAll, { 'status was 200': (r) => r.status == 200 });
    check(requestGetUser, { 'status was 200': (r) => r.status == 200 });
    check(requestDelete, { 'status was 200': (r) => r.status == 200 });
    sleep(1);
}

// k6 run --out influxdb=http://localhost:8086/myk6db tests-k6.js
// ID dashboard grafana: 2587