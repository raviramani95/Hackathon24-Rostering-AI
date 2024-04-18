import { AfterViewInit, Component, OnInit, Renderer2, ViewChild } from '@angular/core';
import { FullCalendarComponent } from '@fullcalendar/angular';
import { CalendarOptions, EventInput } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import dayGridWeek from '@fullcalendar/daygrid';
import interactionPlugin from '@fullcalendar/interaction';

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

  constructor(private renderer: Renderer2){
    this.events = [
      { title: 'event 1', date: '2024-04-16' },
      { title: 'event 2', date: '2024-04-17' }
    ]
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
    dateClick: (arg) => this.handleDateClick(arg),
    events: this.events,
    select: this.clickAndDragCreateJob.bind(this), //date click and new job using drag and drop,
    eventDidMount: this.onEventRender.bind(this),
  };

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

  onEventRender(currentEvent: any){
    const eventDOM = currentEvent.el;

    this.renderer.listen(eventDOM, "click", event => {
    console.log("event render clicked");
    console.log(eventDOM);

    })
  }

  selectedCar: number = 0;

  cars = [
      { id: 1, name: 'Volvo' },
      { id: 2, name: 'Saab' },
      { id: 3, name: 'Opel' },
      { id: 4, name: 'Audi' },
  ];
}
