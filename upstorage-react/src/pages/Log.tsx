
import { useState, useEffect } from 'react'
import {Form, Grid,Container, Header, Input, Segment} from "semantic-ui-react";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import "./LiveLog.css";


const FakeMenu = () => (
  <div className="fakeMenu">
      <div className="fakeButtons fakeClose"></div>
      <div className="fakeButtons fakeMinimize"></div>
      <div className="fakeButtons fakeZoom"></div>
  </div>
);

const FakeScreen = ({ logs }: { logs: LogDto[] }) => (
  <div className="fakeScreen">
    
      {logs.map((log) => (
        
          <p className="line1" >
              {log.Message}
              <span className="cursor1"> <br />{log.SentOn}</span>
          </p>
      ))}
  </div>
);

const LiveLog: React.FC = () => {
  const [logs, setLogs] = useState<LogDto[]>([]);
  const [hubConnection, setHubConnection] = useState<HubConnection | null>(null);

  useEffect(() => {
      const url = "https://localhost:7275/Hubs/UpStorageLogHub";
      const connection = new HubConnectionBuilder()
          .withUrl(url)
          .withAutomaticReconnect()
          .build();

      connection.on("UpStorageLogAdded", (logDto: LogDto) => {
          setLogs((prevLogs) => [...prevLogs, logDto]);
          console.log(logDto.Message)
      });

      async function startConnection() {
          try {
              await connection.start();
              setHubConnection(connection);
          } catch (err) {
              console.error("Error while establishing connection: ", err);
          }
      }

      startConnection();

      return () => {
          if (hubConnection) {
              hubConnection.off("UpStorageLogAdded");
              hubConnection.stop();
          }
      };
  }, []);

  return (
      <>
          <Container>
              <FakeMenu />
              <FakeScreen logs={logs}/>
          </Container>
      </>
  );
};

export default LiveLog