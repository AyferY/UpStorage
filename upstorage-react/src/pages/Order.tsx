import { useState, useEffect } from 'react'
import OrderDropDown from "../components/DropDown/OrderDropDown.tsx";
import {Form, Grid, Header, Input, Segment} from "semantic-ui-react";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import OrderDto from '../types/OrderDto.ts';

function Order() {

  const [orderHubConnection, setOrderHubConnection] = useState<HubConnection | undefined>(undefined);

  useEffect(() =>{

    const startConnection = async () => {
      const connection = new HubConnectionBuilder()
        .withUrl("https://localhost:7275/Hubs/UpStorageOrderHub")
        .withAutomaticReconnect()
        .build();

      await connection.start();

      setOrderHubConnection(connection);
    }

    if(!orderHubConnection){
      startConnection();
    }

  },[])

  const [order, setOrder] = useState<OrderDto>(new OrderDto());

  const [scrapeCount, setScrapeCount] = useState<string>("");

  const [productType, setProductType] = useState<string>("");

    const [showDropDown, setShowDropDown] = useState<boolean>(false);
    const options = () => {
      return ["All", "OnDiscount", "NonDiscount"];
    };
  
    /**
     * Toggle the drop down menu
     */
  

    const toggleDropDown = () => {
      setShowDropDown(!showDropDown);
    };
  
    /**
     * Hide the drop down menu if click occurs
     * outside of the drop-down element.
     *
     * @param event  The mouse event
     */
    const dismissHandler = (event: React.FocusEvent<HTMLButtonElement>): void => {
      if (event.currentTarget === event.target) {
        setShowDropDown(false);
      }
    };
  
    /**
     * Callback function to consume the
     * city name from the child component
     *
     * @param options  The selected city
     */
    const handleProductType = (option: string): void => {
      setProductType(option);
    };

/*test için var*/
  const handleSubmit = async() => {
    console.log(productType);

    order.RequestedAmount = +scrapeCount;
    order.ProductCrawlType = productType;

    await orderHubConnection?.invoke<OrderDto>("AddOrderAsync", order);
  };

  const handleScrapeCount = (value:string) => {
    setScrapeCount(String(value));
  };

  return (
    <>
                <Header as='h2' textAlign='center' color='green' style={{ fontSize: '36px', fontWeight: 'bold' }}>Welcome to UPSTORAGE</Header>
                <Segment raised style={{backgroundColor: '#A2BEB9', boxShadow: '0 8px 16px 0 rgba(0,0,0,0.2)', transition: '0.3s', color: '#173A3A'}}>

                    <Grid >
                            <Grid.Column>
                                <Form>
                                    <Form.Field>
                                        <label>How many items to scrape?</label>
                                        <Input
                                            id="scrapeCountSelector"
                                            value={scrapeCount}
                                            onChange={(_, data) => handleScrapeCount(data.value)}
                                        />
                                    </Form.Field>
                                    <Form.Field>
                                        <label>What products do you want to scraping?</label>
                                      <button
                                        className={showDropDown ? "active" : undefined}
                                        onClick={(): void => toggleDropDown()}
                                        onBlur={(e: React.FocusEvent<HTMLButtonElement>): void =>
                                          dismissHandler(e)
                                        }
                                      >
                                        
                                        <div>{productType ? "Select: " + productType : "Select ..."} </div>
                                        {showDropDown && (
                                          <OrderDropDown
                                            options={options()}
                                            showDropDown={false}
                                            toggleDropDown={(): void => toggleDropDown()}
                                            optionSelection={handleProductType}
                                          />
                                        )}
                                        
                                      </button>
                                    </Form.Field>

                                  <br />
                                  <button onClick={handleSubmit}>Gönder</button>
                                  </Form>
                              </Grid.Column>
                      </Grid>
                  </Segment>
    
    </>
  );
                                        }

export default Order
