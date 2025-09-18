import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class SignalRConnectionFactoryService {
  createSSEConnection(url: string, accessToken: string): signalR.HubConnection {
    return new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl(url, {
        accessTokenFactory: () => accessToken,
        transport: signalR.HttpTransportType.ServerSentEvents,
      })
      .withAutomaticReconnect()
      .build();
  }
}
