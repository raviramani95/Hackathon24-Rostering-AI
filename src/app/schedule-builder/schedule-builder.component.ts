import { AfterViewInit, Component, OnInit, Renderer2, ViewChild } from '@angular/core';
import { FullCalendarComponent } from '@fullcalendar/angular';
import { CalendarOptions, EventInput } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import dayGridWeek from '@fullcalendar/daygrid';
import interactionPlugin from '@fullcalendar/interaction';
import { AppServicesService } from '../services/app-services.service';
import { NgxSpinnerService } from "ngx-spinner";

@Component({
  selector: 'app-schedule-builder',
  templateUrl: './schedule-builder.component.html',
  styleUrls: ['./schedule-builder.component.scss']
})
export class ScheduleBuilderComponent implements OnInit, AfterViewInit {

  title = 'Rostering-AI';
  events: any[] = [];
  @ViewChild("calendar", { static: false })
  fullCalendar: FullCalendarComponent | undefined;

  eventsPromise: Promise<EventInput[]> | undefined;

  constructor(private renderer: Renderer2,
    private autosolveService: AppServicesService,
    private spinner: NgxSpinnerService
  ){
    this.postData();
  }

  calendarOptions: CalendarOptions = {
    initialView: 'dayGridMonth',
    plugins: [dayGridPlugin, dayGridWeek, interactionPlugin],
    themeSystem: 'bootstrap5',
    headerToolbar: {
      left: 'prev,next',
      center: 'title',
      right: 'dayGridDay,dayGridWeek,dayGridMonth'
    },
    editable: true,
    selectable: true,
    //dateClick: (arg) => this.handleDateClick(arg),
    events: this.events,
    eventContent: this.customEventContent.bind(this)
  }

  ngOnInit(): void {
    //
  }

  ngAfterViewInit(): void {
    //this.fullCalendar?.getApi().render();
    this.calendarOptions.events = this.events;
  }

  handleDateClick(arg: any) {
    //xalert('date click! ' + arg.dateStr)
  }

  clickAndDragCreateJob(event: any) {
    console.log("Date clicked");
    console.log(event.startStr);
  }

  // onEventRender(currentEvent: any){
  //   const eventDOM = currentEvent.el;

  //   var tooltip = new Tooltip(currentEvent.el, {
  //     title: currentEvent.event.extendedProps.description,
  //     placement: 'top',
  //     trigger: 'hover',
  //     container: 'body'
  //   });

  //   this.renderer.listen(eventDOM, "click", event => {
  //   console.log("event render clicked");
  //   console.log(eventDOM);

  //   })
  // }

  selectedCar: any;

  customers = [
      { id: 1, name: 'Customer A' },
      { id: 2, name: 'Customer B' },
      { id: 3, name: 'Customer C' }
  ];

  employees: any[] = [];
  scheduleJobs: any[] = [];

  // employees: any = [
  //   {
  //     "Id": 1,
  //     "JobTypeID": 1,
  //     "FirstName": "Josh",
  //     "LastName": "Smith"
  //   },
  //   {
  //     "Id": 2,
  //     "JobTypeID": 1,
  //     "FirstName": "Alex",
  //     "LastName": "James"
  //   },
  //   {
  //     "Id": 3,
  //     "JobTypeID": 1,
  //     "FirstName": "Alex",
  //     "LastName": "Johnson"
  //   },
  //   {
  //     "Id": 4,
  //     "JobTypeID": 2,
  //     "FirstName": "Aaron",
  //     "LastName": "Johnson"
  //   },
  //   {
  //     "Id": 5,
  //     "JobTypeID": 3,
  //     "FirstName": "Joe",
  //     "LastName": "Smith"
  //   },
  //   {
  //     "Id": 6,
  //     "JobTypeID": 3,
  //     "FirstName": "Ian",
  //     "LastName": "Smith"
  //   },
  //   {
  //     "Id": 7,
  //     "JobTypeID": 1,
  //     "FirstName": "Ian",
  //     "LastName": "Johnson"
  //   },
  //   {
  //     "Id": 8,
  //     "JobTypeID": 1,
  //     "FirstName": "James",
  //     "LastName": "Lehman"
  //   },
  //   {
  //     "Id": 9,
  //     "JobTypeID": 1,
  //     "FirstName": "Steven",
  //     "LastName": "Smith"
  //   },
  //   {
  //     "Id": 10,
  //     "JobTypeID": 1,
  //     "FirstName": "Joseph",
  //     "LastName": "Smith"
  //   }

  // ]

  //employees: any[] = [ /* Your employee data */ ];
    selectedEmployees: any[] = [];

    toggleSelection(employee: any) {
        const index = this.selectedEmployees.indexOf(employee);
        if (index > -1) {
            this.selectedEmployees.splice(index, 1);
        } else {
            this.selectedEmployees.push(employee);
        }
        console.log(this.selectedEmployees);
    }

    isSelected(employee: any): boolean {
      //console.log(employee);
        return this.selectedEmployees.indexOf(employee) > -1;
    }

    postData(){
      this.autosolveService.postAutoSolve().subscribe((data: any) => {
       console.log(data);

    },
    (err) => {
        //
    });
    }

    onCustomerSelected(event: any){
      console.log(event);
      this.employees = this.autosolveService.customerData1.Employees;
      this.scheduleJobs =this.autosolveService.customerData1.ScheduleJobs;

      this.events = [];
      this.scheduleJobs.forEach(job => {
        let temp = {
          id: job.id,
          title: job.JobType[0].JobTypeName,
          date: job.JobStartDateTime,
          color: 'green',
          employees: ['Employee 1', 'Employee 2']
        }
        this.events.push(temp);
      });

      this.calendarOptions.events = this.events;
    }

    showSpinner() {
      this.spinner.show();
      setTimeout(() => {
        this.spinner.hide();
      }, 3000);
    }
   
    startSpinner() {
      this.spinner.show();
    }

    // customEventContent(eventInfo: any, createElement: any) {
    //   debugger
    //   const titleHtml = createElement('div', {}, eventInfo.event.title); // Create title element
    //   const employeesHtml = createElement('div', {}, eventInfo.event.extendedProps.employees.join(', ')); // Create employees element
  
    //   const contentHtml = createElement('div', {}, [titleHtml, employeesHtml]); // Combine title and employees
  
    //   return { domNodes: [contentHtml] };
    // }

    // customEventContent(eventInfo: any) {
    //   const container = this.renderer.createElement('div'); // Create a container div
    //   debugger
    //   const titleElement = this.renderer.createElement('div'); // Create a div for the title
    //   const titleText = this.renderer.createText(eventInfo.event.title); // Create text node for the title
    //   this.renderer.appendChild(titleElement, titleText); // Append text to the title div
    //   this.renderer.appendChild(container, titleElement); // Append title div to the container
  
    //   if (eventInfo.event.extendedProps.employees && eventInfo.event.extendedProps.employees.length > 0) {
    //     for(let i=0;i < eventInfo.event.extendedProps.employees.length; i++){
    //       const employeesElement = this.renderer.createElement('div'); // Create a div for the employees
    //       const employeesText = this.renderer.createText(eventInfo.event.extendedProps.employees[i]); // Create text node for employees
    //       this.renderer.appendChild(employeesElement, employeesText); // Append text to the employees div
    //       this.renderer.appendChild(container, employeesElement); // Append employees div to the container
    //     }
        
    //   }
  
    //   return { domNodes: [container] }; // Return container div as the event content
    // }
    customEventContent(eventInfo: any) {
      const container = this.renderer.createElement('div'); // Create a container div
      this.renderer.addClass(container, 'event-container'); // Add a class to the container for styling
  
      const titleElement = this.renderer.createElement('div'); // Create a div for the title
      const titleText = this.renderer.createText(eventInfo.event.title); // Create text node for the title
      this.renderer.appendChild(titleElement, titleText); // Append text to the title div
      this.renderer.appendChild(container, titleElement); // Append title div to the container
  
      if (eventInfo.event.extendedProps.employees && eventInfo.event.extendedProps.employees.length > 0) {
        const employeesBox = this.renderer.createElement('div'); // Create a div for the employees box
        this.renderer.addClass(employeesBox, 'employees-box'); // Add a class to the employees box for styling
  
        for(let i = 0; i < eventInfo.event.extendedProps.employees.length; i++) {
          const employeeElement = this.renderer.createElement('div'); // Create a div for each employee
          const employeeText = this.renderer.createText(eventInfo.event.extendedProps.employees[i]); // Create text node for employee
          this.renderer.appendChild(employeeElement, employeeText); // Append text to the employee div
          this.renderer.appendChild(employeesBox, employeeElement); // Append employee div to the employees box
        }
  
        this.renderer.appendChild(container, employeesBox); // Append employees box to the container
      }
  
      return { domNodes: [container] }; // Return container div as the event content
    }
}
