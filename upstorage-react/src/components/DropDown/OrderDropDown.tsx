import React, { useEffect, useState } from 'react';

type DropDownProps = {
  options: string[];
  showDropDown: boolean;
  toggleDropDown: Function;
  optionSelection: Function;
};

const OrderDropDown: React.FC<DropDownProps> = ({
    options,
    optionSelection,
}: DropDownProps): JSX.Element => {
  const [showDropDown, setShowDropDown] = useState<boolean>(false);

  /**
   * Handle passing the city name
   * back to the parent component
   *
   * @param city  The selected city
   */
  const onClickHandler = (options: string): void => {
    optionSelection(options);
  };

  useEffect(() => {
    setShowDropDown(showDropDown);
  }, [showDropDown]);

  return (
    <>
      <div className={showDropDown ? 'dropdown' : 'dropdown active'}>
        {options.map(
          (options: string, index: number): JSX.Element => {
            return (
              <p
                key={index}
                onClick={(): void => {
                  onClickHandler(options);
                }}
              >
                {options}
              </p>
            );
          }
        )}
      </div>
    </>
  );
};

export default OrderDropDown;