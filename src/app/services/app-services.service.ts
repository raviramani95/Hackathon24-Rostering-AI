import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Observer } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppServicesService {

  apiUrl = 'https://localhost:44337/AutoRoster/autosolve?groupId=1';

  constructor(private http: HttpClient) { }

  postAutoSolve(): Observable<any[]> {
    return new Observable((o: Observer<any>) => {
      this.http.get<any>(this.apiUrl).subscribe((data: any) => {
        o.next(data)
        return o.complete();
      },
        (err: any) => {
          return o.error(err);
        });
    });
  }

  customerData1 = {
    "Employees": [
      {
        "Id": 1,
        "JobType": [
          {
            "Id": 1,
            "JobTypeName": "Supervisor"
          }
        ],
        "FirstName": "Josh",
        "LastName": "Smith"
      },
      {
        "Id": 2,
        "JobType": [
          {
            "Id": 1,
            "JobTypeName": "Supervisor"
          }
        ],
        "FirstName": "Alex",
        "LastName": "James"
      },
      {
        "Id": 3,
        "JobType": [
          {
            "Id": 1,
            "JobTypeName": "Supervisor"
          }
        ],
        "FirstName": "Alex",
        "LastName": "Johnson"
      },
      {
        "Id": 4,
        "JobType": [
          {
            "Id": 2,
            "JobTypeName": "Manager"
          }
        ],
        "FirstName": "Aaron",
        "LastName": "Johnson"
      },
      {
        "Id": 5,
        "JobType": [
          {
            "Id": 3,
            "JobTypeName": "Assembly Worker"
          }
        ],
        "FirstName": "Joe",
        "LastName": "Smith"
      },
      {
        "Id": 6,
        "JobType": [
          {
            "Id": 3,
            "JobTypeName": "Assembly Worker"
          }
        ],
        "FirstName": "Ian",
        "LastName": "Smith"
      },
      {
        "Id": 7,
        "JobType": [
          {
            "Id": 1,
            "JobTypeName": "Supervisor"
          }
        ],
        "FirstName": "Ian",
        "LastName": "Johnson"
      },
      {
        "Id": 8,
        "JobType": [
          {
            "Id": 1,
            "JobTypeName": "Supervisor"
          }
        ],
        "FirstName": "James",
        "LastName": "Lehman"
      },
      {
        "Id": 9,
        "JobType": [
          {
            "Id": 1,
            "JobTypeName": "Supervisor"
          }
        ],
        "FirstName": "Steven",
        "LastName": "Smith"
      },
      {
        "Id": 10,
        "JobType": [
          {
            "Id": 1,
            "JobTypeName": "Supervisor"
          }
        ],
        "FirstName": "Joseph",
        "LastName": "Smith"
      }
  
    ],
    "ScheduleJobs": [
      {
        "Id": 1,
        "JobStartDateTime": "2024-04-08",
        "JobEndDateTime": "12:00-14:00",
        "NoOfEmployeesRequired": 2,
        "JobType": [
          {
            "Id": 1,
            "JobTypeName": "Supervisor"
          }
        ]
      },
      {
        "Id": 2,
        "JobStartDateTime": "2024-04-09",
        "JobEndDateTime": "12:00-14:00",
        "NoOfEmployeesRequired": 3,
        "JobType": [
          {
            "Id": 1,
            "JobTypeName": "Supervisor"
          }
        ]
      },
      {
        "Id": 3,
        "JobStartDateTime": "2024-04-10",
        "JobEndDateTime": "14:00-16:00",
        "NoOfEmployeesRequired": 2,
        "JobType": [
          {
            "Id": 1,
            "JobTypeName": "Supervisor"
          }
        ]
      },
      {
        "Id": 4,
        "JobStartDateTime": "2024-04-11",
        "JobEndDateTime": "14:00-16:00",
        "NoOfEmployeesRequired": 5,
        "JobType": [
          {
            "Id": 2,
            "JobTypeName": "Manager"
          }
        ]
      },
      {
        "Id": 5,
        "JobStartDateTime": "2024-04-12",
        "JobEndDateTime": "14:00-16:00",
        "NoOfEmployeesRequired": 2,
        "JobType": [
          {
            "Id": 3,
            "JobTypeName": "Assembly Worker"
          }
        ]
      },
      {
        "Id": 6,
        "JobStartDateTime": "2024-04-15",
        "JobEndDateTime": "16:00-18:00",
        "NoOfEmployeesRequired": 3,
        "JobType": [
          {
            "Id": 3,
            "JobTypeName": "Assembly Worker"
          }
        ]
      },
      {
        "Id": 7,
        "JobStartDateTime": "2024-04-16",
        "JobEndDateTime": "16:00-18:00",
        "NoOfEmployeesRequired": 7,
        "JobType": [
          {
            "Id": 1,
            "JobTypeName": "Supervisor"
          }
        ]
      },
      {
        "Id": 8,
        "JobStartDateTime": "2024-05-17",
        "JobEndDateTime": "12:00-16:00",
        "NoOfEmployeesRequired": 4,
        "JobType": [
          {
            "Id": 1,
            "JobTypeName": "Supervisor"
          }
        ]
      },
      {
        "Id": 9,
        "JobStartDateTime": "2024-05-18",
        "JobEndDateTime": "12:00-16:00",
        "NoOfEmployeesRequired": 8,
        "JobType": [
          {
            "Id": 1,
            "JobTypeName": "Supervisor"
          }
        ]
      },
      {
        "Id": 10,
        "JobStartDateTime": "2024-05-19",
        "JobEndDateTime": "12:00-16:00",
        "NoOfEmployeesRequired": 6,
        "JobType": [
          {
            "Id": 1,
            "JobTypeName": "Supervisor"
          }
        ]
      }
    ]
  }

  // getPostById(id: number): Observable<any> {
  //   return this.http.get<any>(`${this.apiUrl}/posts/${id}`);
  // }

  // addPost(post: any): Observable<any> {
  //   return this.http.post<any>(`${this.apiUrl}/posts`, post);
  // }

  // updatePost(id: number, post: any): Observable<any> {
  //   return this.http.put<any>(`${this.apiUrl}/posts/${id}`, post);
  // }

  // deletePost(id: number): Observable<any> {
  //   return this.http.delete<any>(`${this.apiUrl}/posts/${id}`);
  // }
}
